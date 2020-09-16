import { observer } from "mobx-react-lite";
import React, { Fragment, useContext, useState } from "react";
import { Button, Grid, Header, Tab } from "semantic-ui-react";
import { IEditFormValues } from "../../app/models/profile";
import { RootStoreContext } from "../../app/stores/rootStore";
import ProfileEditForm from "./ProfileEditForm";

const ProfileDescription = () => {
	const rootStore = useContext(RootStoreContext);
	const { isCurrentUser, profile, editProfile } = rootStore.profileStore;

	const [editProfileMode, setEditProfileMode] = useState(false);

	const handleEditProfile = (values: IEditFormValues) => {
		editProfile(values).then(() => setEditProfileMode(false));
	};
	return (
		<Tab.Pane>
			<Grid>
				<Grid.Column width={16} style={{ paddingBottom: 0 }}>
					<Header floated="left" icon="font" content="About" />
					{isCurrentUser && (
						<Button
							floated="right"
							basic
							content={editProfileMode ? "Cancel" : "Edit"}
							onClick={() => setEditProfileMode(!editProfileMode)}
						/>
					)}
				</Grid.Column>
				<Grid.Column width={16}>
					{editProfileMode ? (
						<ProfileEditForm editProfile={handleEditProfile} />
					) : (
						<Fragment>
							<h1>{profile?.displayName}</h1>
							<p>{profile?.bio}</p>
						</Fragment>
					)}
				</Grid.Column>
			</Grid>
		</Tab.Pane>
	);
};

export default observer(ProfileDescription);
